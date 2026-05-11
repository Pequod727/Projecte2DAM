package com.mc.demo;
/**
 * configura i inicia tot el context de Spring Boot
 */

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

// configuracio automatica de aplicacio spring boot
@SpringBootApplication
public class DemoApplication {

	public static void main(String[] args) {
		// arranca l app passant la classe actual i els arguments de terminal
		SpringApplication.run(DemoApplication.class, args);
	}

}
