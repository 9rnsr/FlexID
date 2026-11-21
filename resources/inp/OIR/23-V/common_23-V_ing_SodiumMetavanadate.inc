[include:common_23-V_parameter.inc]

[parameter]
  # ICRP Publ.151 p.103 Table 12.2 / Ingested materials, Sodium metavanadate

$if parent
  ${V}/fA    = 0.2
  ${V}/fA_Re = fA_MaxValueOfIng     # footnote§
$else
  ${*}/fA    = fA_MaxValueOfIng
  ${*}/fA_Re = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_23-V.inc]
