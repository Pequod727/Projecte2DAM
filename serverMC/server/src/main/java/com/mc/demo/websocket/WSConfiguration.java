package com.mc.demo.websocket;
/**
 * configuracio del punt d'accés endpoint del WebSocket.
 * també estableix a quina URL es connectaran els clients.
 */

import org.springframework.context.annotation.Configuration;
import org.springframework.web.socket.config.annotation.EnableWebSocket;
import org.springframework.web.socket.config.annotation.WebSocketConfigurer;
import org.springframework.web.socket.config.annotation.WebSocketHandlerRegistry;

// aquesta classe es font de ocnfiguracio de Bean
@Configuration
@EnableWebSocket
public class WSConfiguration implements WebSocketConfigurer {
	// gestor de missatges websocket
	private final WSHandler handler;

	// constructor on es passa el handler
	public WSConfiguration(WSHandler handler) {
		this.handler = handler;
	}

	@Override
	public void registerWebSocketHandlers(WebSocketHandlerRegistry registry) {
		// la ruta del handler es /ws i accepta tots els origens (de clients unity)
		registry.addHandler(handler, "/ws").setAllowedOrigins("*");
	}
}