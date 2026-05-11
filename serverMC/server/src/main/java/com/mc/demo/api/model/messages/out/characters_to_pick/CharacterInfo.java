package com.mc.demo.api.model.messages.out.characters_to_pick;

public class CharacterInfo {
    public int characterId;
    public String name;
    public String description;
    public String imageURL;
    public int hp;
    public int atk;
    public int def;
    public int selectedPlayerId;
    public boolean isSelected;

    public CharacterInfo() {}

    public CharacterInfo(int id, String name, String desc, int hp, int atk, int def, String img, int selectedPlayerId, boolean isSelected) {
        this.characterId = id;
        this.name = name;
        this.description = desc;
        this.hp = hp;
        this.atk = atk;
        this.def = def;
        this.imageURL = img;
        this.selectedPlayerId = selectedPlayerId;
        this.isSelected = isSelected;
    }
}