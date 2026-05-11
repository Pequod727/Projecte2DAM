package com.mc.demo.api.model.messages;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class JSONMessage {
    public int gameId;

    @JsonProperty("type")
    public String type;

    public Object data;

    // constructor buit per a jackson
    public JSONMessage() {}

    // constructor manual de 3 parametres
    public JSONMessage(int gameId, String type, Object data) {
        this.gameId = gameId;
        this.type = type;
        this.data = data;
    }

    // constructor de 2 params per objectes MessageBody
    public JSONMessage(int gameId, MessageBody body) {
        this.gameId = gameId;
        this.type = body.getType();
        this.data = body;
    }
}