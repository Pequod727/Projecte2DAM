package com.mc.demo.websocket;


import org.springframework.stereotype.Component;
import org.springframework.web.socket.CloseStatus;
import org.springframework.web.socket.TextMessage;
import org.springframework.web.socket.WebSocketSession;
import org.springframework.web.socket.handler.TextWebSocketHandler;

import com.mc.demo.components.GameManager;

/**
 * classe per gestionar connexions i desconnexions de websocket. tots els missatges rebuts
 * passen per handleTextMessage() i els redirigeix a GameManager
 */

// la classe es un component gestionat per spring
@Component
public class WSHandler extends TextWebSocketHandler {

    private final GameManager gameManager;

    public WSHandler(GameManager gameManager) {
        this.gameManager = gameManager;
    }

	@Override
	protected void handleTextMessage(WebSocketSession session, TextMessage message) {
        // envia el contingut del missatge (payload) al game manager
        gameManager.handleIncoming(session, message.getPayload());
	}

    @Override
    public void afterConnectionEstablished(WebSocketSession session)
    {
        // avisa al gestor que hi ha un nou jugador
        System.out.println("new connection stablished "+session.getId());
        gameManager.onConnect(session);
    }

    @Override
    public void afterConnectionClosed(WebSocketSession session, CloseStatus status){
         // netejar dades al tancar-se una connexio
         System.out.println("connection closed "+session.getId());
         gameManager.onDisconnect(session);
    }
    
}