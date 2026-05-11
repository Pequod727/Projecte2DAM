package com.mc.demo.api.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import com.mc.demo.api.model.entities.ActionEntity;
import org.springframework.stereotype.Repository;

@Repository
public interface ActionRepository extends JpaRepository<ActionEntity, Integer> {
}