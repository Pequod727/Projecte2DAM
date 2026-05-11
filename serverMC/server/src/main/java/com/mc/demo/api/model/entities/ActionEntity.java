package com.mc.demo.api.model.entities;

import jakarta.persistence.*;
import java.util.List;

@Entity
@Table(name = "Accio")
public class ActionEntity {
    @Id
    private int id;
    private String nom;
    private String descripcio;
    private String icona;

    @Column(name = "id_objectiu")
    private int idObjectiu;

    @OneToMany(fetch = FetchType.EAGER)
    @JoinColumn(name = "id_accio")
    private List<EffectEntity> efectes;

    public int getId() { return id; }
    public String getNom() { return nom; }
    public String getDescripcio() { return descripcio; }
    public String getIcona() { return icona; }
    public int getIdObjectiu() { return idObjectiu; }
    public List<EffectEntity> getEfectes() { return efectes; }
}