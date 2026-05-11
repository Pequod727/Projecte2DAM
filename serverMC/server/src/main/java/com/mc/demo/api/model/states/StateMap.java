package com.mc.demo.api.model.states;

import com.mc.demo.api.model.messages.JSONMessage;
import com.mc.demo.api.model.messages.out.show_map.ShowMapMessage_OUT;
import com.mc.demo.components.GameInstance;
import com.mc.demo.components.GameMessage;
import java.util.concurrent.TimeUnit;

public class StateMap extends State {

    public StateMap(GameInstance game) {
        super(game);
        // enviem el missatge per mostrar el mapa a unity
        game.broadcast(new JSONMessage((int)game.getGameId(), new ShowMapMessage_OUT()));
    }

    @Override
    public void tick() {
        // primer esperem a que els jugadors triin un nivell
        GameMessage msg = game.pollMessage(100, TimeUnit.MILLISECONDS);

        if (msg != null) {
            // i llegim el missatge del cient per entrar a un nivell
            System.out.println("Missatge rebut al mapa: " + msg.payload());
        }
    }
}