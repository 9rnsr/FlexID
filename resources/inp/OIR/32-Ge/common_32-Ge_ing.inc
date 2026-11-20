[include:common_32-Ge_parameter.inc]

[parameter]
  # ICRP Publ.151 p.170 Table 18.2 / Ingested materials, All forms

$if parent
  ${Ge}/fA    = 0.99 # ≒ 1
  ${Ge}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_33-As.inc]
[include:element_32-Ge.inc]
[include:element_31-Ga.inc]
