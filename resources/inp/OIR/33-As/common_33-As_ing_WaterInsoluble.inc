[include:common_33-As_parameter.inc]

[parameter]
  # ICRP Publ.151 p.179 Table 19.2 / Ingested materials, Water-Insoluble compounds and arsenic in soil

$if parent
  ${As}/fA    = 0.3
  ${As}/fA_Re = fA_MaxValueOfIng        # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_33-As.inc]
[include:element_32-Ge.inc]
