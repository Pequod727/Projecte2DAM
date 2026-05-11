package com.mc.demo.components;

import java.util.List;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.ExecutorService;
import java.io.IOException;

import org.springframework.web.socket.TextMessage;
import org.springframework.web.socket.WebSocketSession;

import com.mc.demo.api.model.Player;
import com.mc.demo.api.repository.ActionRepository;
import com.mc.demo.api.repository.CharacterRepository;
import com.mc.demo.api.repository.NivellRepository; // AFEGIT
import com.fasterxml.jackson.databind.ObjectMapper;

/**
 * instancia per gestionar cada partida especifica: corren en el seu propi Thread
 * pero nomes es pot executar una partida alhora
 */

public class GameInstance extends Thread {
    private long id;
    private List<Player> players;
    // messages es un blockingqueue que es pot accedir des de multiples fils alhora
    private BlockingQueue<GameMessage> messages = new LinkedBlockingQueue<>();
    private boolean running = true;
    private final ObjectMapper mapper = new ObjectMapper();

    // repositoris per interactuar amb la BD
    private final CharacterRepository characterRepository;
    private final ActionRepository actionRepository;
    private final NivellRepository nivellRepository;

    // estat actual del joc
    private com.mc.demo.api.model.states.State gameState;

    // constructor
    public GameInstance(long id, List<Player> players, ExecutorService executor, CharacterRepository repo, ActionRepository actionRepo, NivellRepository nivellRepo) {
        this.id = id;
        this.players = players;
        this.characterRepository = repo;
        this.actionRepository = actionRepo;
        this.nivellRepository = nivellRepo;
        this.gameState = new com.mc.demo.api.model.states.StateWaiting(this);
    }

    // es crida aquest metode per cada game.start() i fa un nou fil
    @Override
    public void run() {
        System.out.println("Partida " + id + " iniciada.");
        while (running) {
            // executa la lògica de l estat actual en cada tick
            if (gameState != null) gameState.tick();
            try {
                // mini pausa per controlar la velocitat d execució
                Thread.sleep(50);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }

    // el fil principal del servidor afegeix un missatge rebut
    // per websocket a la cua per ser processat pel fil de la partida
    public void enqueue(GameMessage msg) { messages.add(msg); }
    // canvia l estat de la partida
    public void setState(com.mc.demo.api.model.states.State newState) { this.gameState = newState; }
    // extreu un missatge de la cua; si esta buida, s espera dormit.
    // es posa un timeout perque no es bloqueji i segueixi comprovant
    public GameMessage pollMessage(long timeout, TimeUnit unit) {
        try { return messages.poll(timeout, unit); } catch (InterruptedException e) { return null; }
    }

    // getter pels repositoris, jugadors..
    public long getGameId() { return id; }
    public List<Player> getPlayers() { return players; }
    public CharacterRepository getCharacterRepository() { return characterRepository; }
    public ActionRepository getActionRepository() { return actionRepository; }
    public NivellRepository getNivellRepository() { return nivellRepository; }

    // envia un missatge a tothom
    public void broadcast(Object msg) {
        for (Player p : players) send(p.getSession(), msg);
    }

    // envia un missatge a un jugador (session) especific
    public void send(WebSocketSession session, Object msg) {
        sendStatic(session, msg);
    }

    // metode estàtic per enviar JSON per WebSocket
    public static void sendStatic(WebSocketSession session, Object msg) {
        if (session != null && session.isOpen()) {
            try {
                ObjectMapper staticMapper = new ObjectMapper();
                String json = staticMapper.writeValueAsString(msg);
                session.sendMessage(new TextMessage(json));
            } catch (IOException e) {
                System.err.println("Error enviant JSON: " + e.getMessage());
            }
        }
    }
}