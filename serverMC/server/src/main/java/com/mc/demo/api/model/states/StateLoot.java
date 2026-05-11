package com.mc.demo.api.model.states;

import com.mc.demo.api.model.messages.JSONMessage;
import com.mc.demo.api.model.messages.MessagePayload;
import com.mc.demo.components.GameInstance;
import com.mc.demo.components.GameMessage;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.util.List;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.TimeUnit;

public class StateLoot extends State {
    private final ObjectMapper mapper = new ObjectMapper();
    private final ConcurrentHashMap<Integer, Long> claimedItems = new ConcurrentHashMap<>();

    public StateLoot(GameInstance game) {
        super(game);
        System.out.println("[LOOT] Iniciant fase de botí.");

        MessagePayload p = new MessagePayload();

        MessagePayload.ItemDTO item = new MessagePayload.ItemDTO();
        item.itemId = 101;
        item.name = "Espasa de Foc";
        item.iconUrl = "https://api.icon/items/fire_sword.png";

        p.availableItems = List.of(item);

        game.broadcast(new JSONMessage((int)game.getGameId(), "LOOT_START", p));
    }

    @Override
    public void tick() {
        GameMessage msg = game.pollMessage(100, TimeUnit.MILLISECONDS);
        if (msg == null) return;

        try {
            var node = mapper.readTree(msg.payload());
            if ("TAKE_LOOT".equals(node.get("type").asText())) {
                int itemId = node.get("data").get("itemId").asInt();
                resolveClaim(msg.player().getId(), itemId);
            }
        } catch (Exception e) {
            System.err.println("[LOOT ERROR] " + e.getMessage());
        }
    }

    private synchronized void resolveClaim(long playerId, int itemId) {
        if (!claimedItems.containsKey(itemId)) {
            claimedItems.put(itemId, playerId);

            MessagePayload res = new MessagePayload();
            res.itemId = itemId;
            res.winnerPlayerId = playerId;
            res.wasSuccessful = true;

            game.broadcast(new JSONMessage((int)game.getGameId(), "LOOT_RESULT", res));
        }
    }
}