package com.mc.demo.api.model.states;

import java.util.List;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

import com.mc.demo.api.model.Player;
import com.mc.demo.api.model.messages.JSONMessage;
import com.mc.demo.api.model.messages.in.pick_characters.PickCharacterMessage_IN;
import com.mc.demo.api.model.messages.out.characters_to_pick.*;
import com.mc.demo.components.GameInstance;
import com.mc.demo.components.GameMessage;
import com.fasterxml.jackson.databind.ObjectMapper;

@SuppressWarnings("SpellCheckingInspection")
public class StatePickCharacter extends State {
    private final ObjectMapper mapper = new ObjectMapper();
    private Players2SelectMessage_OUT m;

    public StatePickCharacter(GameInstance game) {
        super(game);

        List<PlayerInfo> playerInfos = game.getPlayers().stream()
                .map(p -> new PlayerInfo(p.getId(), "Jugador " + p.getId()))
                .collect(Collectors.toList());

        List<CharacterInfo> charactersFromDB = game.getCharacterRepository().findByJugableTrue().stream()
                .map(entity -> new CharacterInfo(
                        entity.getId(),
                        entity.getNom(),
                        entity.getDescripcio(),
                        entity.getVida(),
                        entity.getAtac(),
                        entity.getDefensa(),
                        entity.getImatge(),
                        -1,
                        false
                ))
                .collect(Collectors.toList());

        m = new Players2SelectMessage_OUT();
        m.personatges = charactersFromDB;
        m.players = playerInfos;

        game.broadcast(new JSONMessage((int)game.getGameId(), "CHARACTER_SELECTION", m));
    }

    @Override
    public void tick() {
        GameMessage message = game.pollMessage(100, TimeUnit.MILLISECONDS);
        if (message != null) {
            try {
                JSONMessage gm = mapper.readValue(message.payload(), JSONMessage.class);
                if ("PICK_CHARACTER".equals(gm.type)) {
                    processPick(message.player(), gm);
                }
            } catch (Exception e) {
                System.err.println("Error en StatePickCharacter: " + e.getMessage());
            }
        }
    }

    private void processPick(Player p, JSONMessage jsonMsg) {
        try {
            PickCharacterMessage_IN req = mapper.convertValue(jsonMsg.data, PickCharacterMessage_IN.class);
            m.personatges.stream()
                    .filter(c -> c.characterId == req.id && !c.isSelected)
                    .findFirst()
                    .ifPresent(c -> {
                        c.selectedPlayerId = (int) p.getId();
                        c.isSelected = true;
                        if (m.personatges.stream().filter(pc -> pc.isSelected).count() < game.getPlayers().size()) {
                            game.broadcast(new JSONMessage((int)game.getGameId(), "CHARACTER_SELECTION", m));
                        } else {
                            // Cridem al combat dient-li que carregui l'ordre 1 (Tutorial)
                            game.setState(new StateBattle(game, m.personatges, 1));
                        }
                    });
        } catch (Exception e) {
            System.err.println("Error processant selecció: " + e.getMessage());
        }
    }
}