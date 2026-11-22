[include:common_35-Br_parameter.inc]

[parameter]
  # ICRP Publ.151 p.198 Table 21.3 / Ingested materials, All forms

$if parent
  ${Br}/fA    = 0.99 # ≒1
  ${Br}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_36-Kr.inc]
[include:element_35-Br.inc]
[include:element_34-Se.inc]
