[parameter]
  # ICRP Publ.134 p.21 Table 2.3 / Ingested materials, Soluble forms

  ${H}/fA    = 0.99 # ≒1
  ${H}/fA_Re = 0

  $Blood = 'mix-Blood'


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_1-H_OBT.inc]
