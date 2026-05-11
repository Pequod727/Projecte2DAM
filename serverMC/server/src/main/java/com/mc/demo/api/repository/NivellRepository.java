package com.mc.demo.api.repository;

import com.mc.demo.api.model.entities.NivellEntity;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface NivellRepository extends JpaRepository<NivellEntity, Integer> {
    NivellEntity findByOrdre(int ordre);
}