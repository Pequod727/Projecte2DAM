package com.mc.demo.api.model.messages.out.show_map;

import com.mc.demo.api.model.messages.MessageBody;

public class ShowMapMessage_OUT extends MessageBody {
    public static final String TYPE = "SHOW_MAP";

    @Override
    public String getType() {
        return TYPE;
    }
}