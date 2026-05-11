package com.mc.demo.api.model.entities;

import jakarta.persistence.*;

@Entity
@Table(name = "Nivell")
public class NivellEntity {

    @Id
    private Integer id;

    private Integer ordre;
    private String fons;

    @Column(name = "id_enemic_1")
    private Integer idEnemic1;

    @Column(name = "id_enemic_2")
    private Integer idEnemic2;

    @Column(name = "id_enemic_3")
    private Integer idEnemic3;

    // getters i setters
    public Integer getId() { return id; }
    public void setId(Integer id) { this.id = id; }

    public Integer getOrdre() { return ordre; }
    public void setOrdre(Integer ordre) { this.ordre = ordre; }

    public String getFons() { return fons; }
    public void setFons(String fons) { this.fons = fons; }

    public Integer getIdEnemic1() { return idEnemic1; }
    public void setIdEnemic1(Integer idEnemic1) { this.idEnemic1 = idEnemic1; }

    public Integer getIdEnemic2() { return idEnemic2; }
    public void setIdEnemic2(Integer idEnemic2) { this.idEnemic2 = idEnemic2; }

    public Integer getIdEnemic3() { return idEnemic3; }
    public void setIdEnemic3(Integer idEnemic3) { this.idEnemic3 = idEnemic3; }
}