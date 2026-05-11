package com.mc.demo.api.model.messages.out.generic;

import com.mc.demo.api.model.messages.MessageBody;

public class ActionResult_OUT extends MessageBody {
    // el tipus de msg que unity espera rebre
    public static final String TYPE = "ACTION_RESULT";

    public boolean success;
    public int errorCode;

    public ActionResult_OUT(boolean success, int errorCode) {
        this.success = success;
        this.errorCode = errorCode;
    }

    // implementar el que demana MessageBody
    @Override
    public String getType() {
        return TYPE;
    }
}