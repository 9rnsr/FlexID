# ICRP Publ.134 p.110 Para.217
# > (217) The only radioactive progeny of a sulphur isotope addressed in this publication
# > is 38Cl (t½=37.24 min), produced by decay of 38S. It is assumed for dosimetric
# > purposes that 38Cl decays at its site of production in the body.
# > (217) 本稿で扱う硫黄同位体の放射性子孫核種は、38Sの崩壊によって生成される38Cl（t½=37.24分）
# > のみである。線量評価の目的上、38Clは体内で生成した部位で崩壊すると仮定する。

[{Cl}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------

# ICRP Publ.134 p.109 Fig.5.2 organic sulpher
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   AllTissues            Other
  exc   Excreta(hair)         ---


[{Cl}:transfer]
  {S}/Blood1                Blood1                    ---
  {S}/Blood2                Blood2                    ---
  {S}/AllTissues            AllTissues                ---
  {S}/Excreta(hair)         Excreta(hair)             ---
