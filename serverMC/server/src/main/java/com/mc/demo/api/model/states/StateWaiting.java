package com.mc.demo.api.model.states;

import com.mc.demo.components.GameInstance;
import com.mc.demo.components.GameMessage;
import java.util.concurrent.TimeUnit;

public class StateWaiting extends State {

    public StateWaiting(GameInstance game) {
        super(game);
        System.out.println("⏳ Estat: Esperant jugadors...");
    }

    @Override
    public void tick() {
        // miren si hi ha algun missatge i esperem un segon
        game.pollMessage(1, TimeUnit.SECONDS);

        // quan s uneixen 2 o mes jugadors canviem el estat a la seleccio de personatges
        if (game.getPlayers().size() >= 2) {
            game.setState(new StatePickCharacter(game));
        }
    }
}