package com.mc.demo.components;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.springframework.web.socket.WebSocketSession;

import com.mc.demo.api.model.Player;
import com.mc.demo.api.model.messages.JSONMessage;
import com.mc.demo.api.repository.ActionRepository;
import com.mc.demo.api.repository.CharacterRepository;
import com.mc.demo.api.repository.NivellRepository; // AFEGIT

/**
 * lobby del sistema, gestiona qui esta connectat i decideix quan es crea
 * una nova instancia (partida) quan hi ha suficients jugadors
 */

@Component
public class GameManager {

    // repositoris per accedir a la BD
    private final CharacterRepository characterRepository;
    private final ActionRepository actionRepository;
    private final NivellRepository nivellRepository;

    // insereix els repositoris automaticament
    @Autowired
    public GameManager(CharacterRepository characterRepository, ActionRepository actionRepository, NivellRepository nivellRepository) {
        this.characterRepository = characterRepository;
        this.actionRepository = actionRepository;
        this.nivellRepository = nivellRepository;
    }

    // mapes per localitzar els jugadors i partides
    private final Map<String, Player> players = new ConcurrentHashMap<>();
    private final Map<Long, GameInstance> games = new ConcurrentHashMap<>();

    // ExecutorService es un grup de fils que els gestiona sol. newCachedThreadPool
    // crea fils nous quan es necessiten i reutilitza els que ya han aacaba
    private final ExecutorService executor = Executors.newCachedThreadPool();
    private long nextGameId = 1;

    public void onConnect(WebSocketSession session) {
        // crea un nou jugador quan s obre el socket
        Player p = new Player(players.size() + 1, session);
        players.put(session.getId(), p);

        System.out.println("Nou jugador connectat: " + p.getId());

        // si es el primer en entrar esta en waiting for players esperant un minim de 2
        if (players.size() == 1) {
            GameInstance.sendStatic(session, new JSONMessage(0, "WAITING_FOR_PLAYERS", null));
        }

        // si son suficients i no hi ha partides actives es crea una
        if (players.size() >= 2 && games.isEmpty()) {
            List<Player> playersToJoin = new ArrayList<>(players.values());
            createGame(playersToJoin);
        }
    }

    private void createGame(List<Player> playersList) {
        long gameId = nextGameId++;
        // instancia la logica de la partida i la inicia com un nou fil
        GameInstance game = new GameInstance(gameId, playersList, executor, characterRepository, actionRepository, nivellRepository);
        games.put(gameId, game);
        // crida el metode run() en un nou fil
        game.start();
        System.out.println("Partida " + gameId + " creada.");
    }

    public void handleIncoming(WebSocketSession session, String payload) {
        // busca a quina partida pertany el jugador que ha enviat el missatge
        Player p = players.get(session.getId());
        if (p != null) {
            for (GameInstance game : games.values()) {
                if (game.getPlayers().contains(p)) {
                    // envia el missatge a la cua corresponent
                    game.enqueue(new GameMessage(p, payload));
                    break;
                }
            }
        }
    }

    public void onDisconnect(WebSocketSession session) {
        players.remove(session.getId());
        System.out.println("Jugador desconnectat: " + session.getId());
    }
}