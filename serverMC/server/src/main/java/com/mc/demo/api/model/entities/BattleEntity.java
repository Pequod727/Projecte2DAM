package com.mc.demo.api.model.entities;

import com.mc.demo.api.model.messages.MessagePayload;
import java.util.*;

public class BattleEntity {
    public int id;
    public String name;
    public int hp, maxHp, atk, def, vel;
    public boolean isPlayer;
    public String imatge; // Aquí rebrem la URL completa de la BD

    // gestor d estats actius
    public static class ActiveStatus {
        public int duration;
        public int quantity;
        public ActiveStatus(int duration, int quantity) {
            this.duration = duration;
            this.quantity = quantity;
        }
    }
    public Map<Integer, ActiveStatus> activeStatuses = new HashMap<>();

    public BattleEntity(int id, String name, int hp, int atk, int def, int vel, boolean isPlayer, String imatge) {
        this.id = id;
        this.name = name;
        this.hp = hp;
        this.maxHp = hp;
        this.atk = atk;
        this.def = def;
        this.vel = vel;
        this.isPlayer = isPlayer;
        this.imatge = imatge;
    }

    public boolean isDead() { return hp <= 0; }

    // per netejar el stateBattle
    public MessagePayload.EntityDTO toDTO() {
        MessagePayload.EntityDTO dto = new MessagePayload.EntityDTO();
        dto.id = this.id;
        dto.nom = this.name;
        dto.hp = this.hp;
        dto.maxHp = this.maxHp;
        dto.equip = isPlayer ? 1 : 2;

        // convertim el mapa a nova llista d objectes
        dto.statusEffects = new ArrayList<>();
        for (Integer statusId : activeStatuses.keySet()) {
            MessagePayload.StatusDTO s = new MessagePayload.StatusDTO();
            s.id = statusId;
            s.name = "Estat " + statusId;
            s.iconUrl = "";
            dto.statusEffects.add(s);
        }

        // passem la URL de la BD a unity directament
        if (this.imatge != null && !this.imatge.isEmpty()) {
            dto.imageUrl = this.imatge;
        } else {
            dto.imageUrl = "https://via.placeholder.com/150";
        }

        return dto;
    }
}