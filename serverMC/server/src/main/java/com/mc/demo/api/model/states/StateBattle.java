package com.mc.demo.api.model.states;

import com.mc.demo.api.model.entities.BattleEntity;
import com.mc.demo.api.model.entities.NivellEntity;
import com.mc.demo.api.model.messages.JSONMessage;
import com.mc.demo.api.model.messages.MessagePayload;
import com.mc.demo.api.model.messages.out.characters_to_pick.CharacterInfo;
import com.mc.demo.components.GameInstance;
import com.mc.demo.components.GameMessage;
import com.fasterxml.jackson.databind.ObjectMapper;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

public class StateBattle extends State {
    private final ObjectMapper mapper = new ObjectMapper();
    private final List<BattleEntity> entities = new ArrayList<>();
    private List<Integer> turnOrder = new ArrayList<>();
    private int currentTurnIndex = 0;
    private final Set<Long> readyPlayers = new HashSet<>();
    private boolean battleStarted = false;

    private final int ordreNivellActual;
    private int generadorIdEnemic = 1000;

    public StateBattle(GameInstance game, List<CharacterInfo> team, int ordreNivell) {
        super(game);
        this.ordreNivellActual = ordreNivell;
        initBattle(team, ordreNivell);
    }

    private void initBattle(List<CharacterInfo> team, int ordreNivell) {
        MessagePayload p = new MessagePayload();
        p.participants = new ArrayList<>();

        NivellEntity nivellActual = game.getNivellRepository().findByOrdre(ordreNivell);

        if (nivellActual == null) {
            System.err.println("[ERROR] No s'ha trobat el nivell a la BD amb ordre: " + ordreNivell);
            return;
        }

        p.levelBackgroundImage = nivellActual.getFons();

        for (CharacterInfo c : team) {
            if (c.isSelected) {
                game.getCharacterRepository().findById(c.characterId).ifPresent(entity -> {
                    entities.add(new BattleEntity(entity.getId(), entity.getNom(), entity.getVida(), entity.getAtac(), entity.getDefensa(), entity.getVelocitat(), true, ""));
                    MessagePayload.EntityDTO dto = new MessagePayload.EntityDTO();
                    dto.id = entity.getId();
                    dto.nom = entity.getNom();
                    dto.imageUrl = entity.getImatge();
                    dto.hp = entity.getVida();
                    dto.maxHp = entity.getVida();
                    dto.equip = 1;
                    dto.statusEffects = new ArrayList<>();
                    p.participants.add(dto);
                });
            }
        }

        carregarEnemicDelNivell(nivellActual.getIdEnemic1(), p.participants);
        carregarEnemicDelNivell(nivellActual.getIdEnemic2(), p.participants);
        carregarEnemicDelNivell(nivellActual.getIdEnemic3(), p.participants);

        turnOrder = entities.stream().sorted((a, b) -> Integer.compare(b.vel, a.vel)).map(e -> e.id).collect(Collectors.toList());
        p.turnOrder = turnOrder;

        game.broadcast(new JSONMessage((int)game.getGameId(), "BATTLE_START", p));
    }

    private void carregarEnemicDelNivell(Integer idEnemicBD, List<MessagePayload.EntityDTO> participantsJson) {
        if (idEnemicBD == null) return;

        game.getCharacterRepository().findById(idEnemicBD).ifPresent(enemicBD -> {
            int idCombat = generadorIdEnemic++;

            entities.add(new BattleEntity(idCombat, enemicBD.getNom(), enemicBD.getVida(), enemicBD.getAtac(), enemicBD.getDefensa(), enemicBD.getVelocitat(), false, ""));

            MessagePayload.EntityDTO dto = new MessagePayload.EntityDTO();
            dto.id = idCombat;
            dto.nom = enemicBD.getNom();
            dto.imageUrl = enemicBD.getImatge();
            dto.hp = enemicBD.getVida();
            dto.maxHp = enemicBD.getVida();
            dto.equip = 2;
            dto.statusEffects = new ArrayList<>();
            participantsJson.add(dto);
        });
    }

    private void startTurn() {
        if (checkVictoryOrDefeat()) return;

        int activeId = turnOrder.get(currentTurnIndex);
        BattleEntity active = getEntity(activeId);
        if (active == null) return;

        MessagePayload p = new MessagePayload();
        p.activeEntityId = activeId;
        p.statusMessage = "Torn de " + active.name;

        if (active.isPlayer) {
            p.availableActions = new ArrayList<>();
            MessagePayload.ActionDTO a1 = new MessagePayload.ActionDTO();
            a1.actionId = 10; a1.name = "Cop Sagrat"; a1.isItem = false; a1.requiresTarget = true;
            p.availableActions.add(a1);

            MessagePayload.ActionDTO it = new MessagePayload.ActionDTO();
            it.actionId = 50; it.name = "Poció X"; it.isItem = true; it.requiresTarget = false;
            p.availableActions.add(it);
        }

        game.broadcast(new JSONMessage((int)game.getGameId(), "START_TURN", p));

        if (active.isDead()) {
            nextTurn();
        } else if (!active.isPlayer) {
            executeAI(active);
        }
    }

    private void executeAI(BattleEntity ai) {
        entities.stream().filter(e -> e.isPlayer && !e.isDead()).findFirst().ifPresent(target -> new Thread(() -> {
            try { Thread.sleep(1000); } catch (InterruptedException ignored) {}
            resolveAction(ai, target, 0);
        }).start());
    }

    private void resolveAction(BattleEntity source, BattleEntity target, int actionId) {
        if (source == null) return;

        MessagePayload res = new MessagePayload();
        res.attackerId = source.id;
        res.actionId = actionId;
        res.actionName = "Acció " + actionId;

        // inicialitzem les llistes
        res.appliedStatuses = new ArrayList<>();
        res.removedStatuses = new ArrayList<>();

        if (target != null) {
            // si hi ha un objectiu
            int dmg = Math.max(5, source.atk - target.def);
            target.hp = Math.max(0, target.hp - dmg);

            res.targetId = target.id;
            res.damageDone = dmg;
            res.currentHp = target.hp;
            res.targetIsDead = target.hp <= 0;

            res.logMessagePersonal = "Has rebut " + dmg + " punts de dany!";
            res.logMessageGlobal = target.name + " ha rebut " + dmg + " punts de dany de " + source.name + "!";
        } else {
            // si no hi ha objectiu (un mateix)
            res.targetId = source.id;
            res.damageDone = 0;
            res.currentHp = source.hp;
            res.targetIsDead = source.hp <= 0;

            res.logMessagePersonal = "Has utilitzat " + res.actionName + "!";
            res.logMessageGlobal = source.name + " ha utilitzat " + res.actionName + "!";
        }

        System.out.println("[BATTLE LOG] Enviant ACTION_RESULT -> Attacker: " + res.attackerId + " | Target: " + res.targetId + " | Dany: " + res.damageDone);
        game.broadcast(new JSONMessage((int)game.getGameId(), "ACTION_RESULT", res));

        nextTurn();
    }

    private void nextTurn() {
        currentTurnIndex = (currentTurnIndex + 1) % turnOrder.size();
        try { Thread.sleep(500); } catch (InterruptedException ignored) {}
        startTurn();
    }

    @Override
    public void tick() {
        GameMessage msg = game.pollMessage(100, TimeUnit.MILLISECONDS);
        if (msg == null) return;
        try {
            var node = mapper.readTree(msg.payload());
            if (!node.has("type")) return;
            String type = node.get("type").asText();
            switch (type) {
                case "CLIENT_READY":
                    readyPlayers.add(msg.player().getId());
                    if (!battleStarted && readyPlayers.size() == game.getPlayers().size()) {
                        battleStarted = true;
                        startTurn();
                    }
                    break;
                case "PLAYER_ACTION":
                    if (battleStarted) {
                        int tid = node.get("data").get("targetId").asInt();
                        int aid = node.get("data").get("actionId").asInt();
                        resolveAction(getEntity((int)msg.player().getId()), (tid == 0) ? null : getEntity(tid), aid);
                    }
                    break;
            }
        } catch (Exception e) { System.err.println("Error combat: " + e.getMessage()); }
    }

    // comprovem si ha mort un jugador o enemic
    private boolean checkVictoryOrDefeat() {
        boolean enemiesDead = entities.stream().filter(e -> !e.isPlayer).allMatch(BattleEntity::isDead);
        if (enemiesDead) {
            System.out.println("[SISTEMA] Nivell " + ordreNivellActual + " superat!");
            game.setState(new StateLoot(game));
            return true;
        }

        boolean playersDead = entities.stream().filter(e -> e.isPlayer).allMatch(BattleEntity::isDead);
        if (playersDead) {
            System.out.println("[SISTEMA] Tots els jugadors han mort. GAME OVER.");
            // per programar - game.setState(new StateGameOver(game));
            return true;
        }

        return false;
    }

    private BattleEntity getEntity(int id) { return entities.stream().filter(e -> e.id == id).findFirst().orElse(null); }
}