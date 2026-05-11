package com.mc.demo.api.model.entities;

import jakarta.persistence.*;

@Entity
@Table(name = "Efecte")
public class EffectEntity {
    @Id
    private int id;
    private String nom;
    private int probabilitat;
    private Integer quantitat;
    private int duracio;
    private boolean esAfegir;

    @Column(name = "id_estadistica")
    private Integer idEstadistica;

    @Column(name = "id_estat")
    private Integer idEstat;

    public int getId() { return id; }
    public String getNom() { return nom; }
    public Integer getIdEstadistica() { return idEstadistica; }
    public Integer getIdEstat() { return idEstat; }
    public Integer getQuantitat() { return quantitat; }
    public boolean isEsAfegir() { return esAfegir; }
    public int getProbabilitat() { return probabilitat; }
    public int getDuracio() { return duracio; }
}