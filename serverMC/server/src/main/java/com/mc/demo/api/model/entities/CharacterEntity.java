package com.mc.demo.api.model.entities;

import jakarta.persistence.*;
import java.util.List;

@Entity
@Table(name = "Personatge")
public class CharacterEntity {

    @Id
    private int id;
    private String nom;
    private String descripcio;
    private int atac;
    private int defensa;
    private int vida;
    private int velocitat;
    private int experiencia;
    private String imatge;
    private String icona;
    private boolean jugable;

    @ManyToMany(fetch = FetchType.EAGER)
    @JoinTable(
            name = "Habilitat",
            joinColumns = @JoinColumn(name = "id_personatge"),
            inverseJoinColumns = @JoinColumn(name = "id_accio")
    )
    private List<ActionEntity> habilitats;

    public int getId() { return id; }
    public String getNom() { return nom; }
    public String getDescripcio() { return descripcio; }
    public int getAtac() { return atac; }
    public int getDefensa() { return defensa; }
    public int getVida() { return vida; }
    public int getVelocitat() { return velocitat; }
    public int getExperiencia() { return experiencia; }
    public String getImatge() { return imatge; }
    public String getIcona() { return icona; }
    public boolean isJugable() { return jugable; }
    public List<ActionEntity> getHabilitats() { return habilitats; }

    public void setId(int id) { this.id = id; }
    public void setNom(String nom) { this.nom = nom; }
    public void setDescripcio(String d) { this.descripcio = d; }
    public void setAtac(int a) { this.atac = a; }
    public void setDefensa(int d) { this.defensa = d; }
    public void setVida(int v) { this.vida = v; }
    public void setVelocitat(int v) { this.velocitat = v; }
    public void setExperiencia(int e) { this.experiencia = e; }
    public void setImatge(String i) { this.imatge = i; }
    public void setIcona(String i) { this.icona = i; }
    public void setJugable(boolean j) { this.jugable = j; }
    public void setHabilitats(List<ActionEntity> habilitats) { this.habilitats = habilitats; }
}