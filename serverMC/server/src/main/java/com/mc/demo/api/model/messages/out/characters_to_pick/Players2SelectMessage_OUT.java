package com.mc.demo.api.model.messages.out.characters_to_pick;

import com.mc.demo.api.model.messages.MessageBody;
import java.util.List;

public class Players2SelectMessage_OUT extends MessageBody {
    public static final String TYPE = "CHARACTER_SELECTION";

    public List<CharacterInfo> personatges;
    public List<PlayerInfo> players;

    @Override
    public String getType() {
        return TYPE;
    }
}