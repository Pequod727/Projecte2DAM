package com.mc.demo.api.model.messages.in.pick_characters;

import com.mc.demo.api.model.messages.MessageBody;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public class PickCharacterMessage_IN extends MessageBody {
    public static final String TYPE = "PICK_CHARACTER";

    public int id;
    public int playerId;

    @Override
    public String getType() {
        return TYPE;
    }
}