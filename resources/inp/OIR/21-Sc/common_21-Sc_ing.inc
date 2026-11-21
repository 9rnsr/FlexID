[include:common_21-Sc_parameter.inc]

[parameter]
  # ICRP Publ.151 p.88 Table 10.2 / Ingested materials, All forms

$if parent
  ${Sc}/fA    = 0.001
  ${Sc}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_21-Sc.inc]
