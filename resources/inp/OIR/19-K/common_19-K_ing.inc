[include:common_19-K_parameter.inc]

[parameter]
  # ICRP Publ.151 p.79 Table 9.2 / Ingested materials, All forms

$if parent
  ${K}/fA    = 0.99 # ≒1
  ${K}/fA_Re = fA_MaxValueOfIng     # footnote§
$else
  ${*}/fA    = fA_MaxValueOfIng
  ${*}/fA_Re = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_20-Ca.inc]
[include:element_19-K.inc]
