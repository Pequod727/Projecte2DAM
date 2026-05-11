package com.mc.demo.api.model.messages;

import com.fasterxml.jackson.annotation.JsonInclude;
import java.util.List;

@JsonInclude(JsonInclude.Include.NON_NULL)
public class MessagePayload {
    public String statusMessage;

    // lobby
    public List<Object> players;
    public List<Object> personatges;

    // combat
    public String levelBackgroundImage;
    public List<EntityDTO> participants;
    public List<Integer> turnOrder;

    // torns i estats
    public Integer activeEntityId;
    public Integer preTurnDamage;
    public Boolean skipTurn;
    public String statusLog;

    // resultats d accions
    public Integer attackerId;
    public Integer targetId;
    public Integer actionId;
    public String actionName;
    public Integer damageDone;
    public Integer healAmount;
    public Integer currentHp;
    public Boolean targetIsDead;
    public String logMessagePersonal;
    public String logMessageGlobal;

    // ids per les animacions de unity
    public List<Integer> appliedStatuses;
    public List<Integer> removedStatuses;

    // variables de menus
    public List<ActionDTO> availableActions;

    // fase de loot

    public List<ItemDTO> availableItems;
    public Integer itemId;
    public Long winnerPlayerId;
    public Boolean wasSuccessful;

    // dtos del servidor
    public static class EntityDTO {
        public int id;
        public String nom;
        public String imageUrl;
        public int hp, maxHp, equip;
        public List<StatusDTO> statusEffects; // icones de la interficie
    }

    public static class StatusDTO {
        public int id;
        public String name;
        public String iconUrl;
    }

    public static class ItemDTO {
        public int itemId;
        public String name;
        public String iconUrl;
    }

    public static class ActionDTO {
        public int actionId;
        public String name;
        public boolean isItem;
        public boolean requiresTarget;
    }
}