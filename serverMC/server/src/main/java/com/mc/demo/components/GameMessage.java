package com.mc.demo.components;

import com.mc.demo.api.model.Player;

/**
 * classe per empaquetar missatge junt amb el seu autor
 * player (autor que ho envia) i payload (contingut en JSON)
 */

public record GameMessage(Player player, String payload) {}