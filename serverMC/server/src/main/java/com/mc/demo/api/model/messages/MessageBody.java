package com.mc.demo.api.model.messages;

import com.fasterxml.jackson.annotation.JsonIgnore;

public abstract class MessageBody {
    // tots els missatges tenen aquest metode
    @JsonIgnore
    public abstract String getType();
}