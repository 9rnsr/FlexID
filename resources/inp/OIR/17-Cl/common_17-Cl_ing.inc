[include:common_17-Cl_parameter.inc]

[parameter]
  # ICRP Publ.151 p.74 Table 8.3 / Ingested materials, All forms

$if parent
  ${Cl}/fA    = 0.99 # ≒1
  ${Cl}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_18-Ar.inc]
[include:element_17-Cl.inc]
