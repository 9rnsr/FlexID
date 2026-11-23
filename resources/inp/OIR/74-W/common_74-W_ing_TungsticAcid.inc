[include:common_74-W_parameter.inc]

[parameter]
  # ICRP Publ.151 p.280 Table 31.2 / Ingested materials, Tungstic acid

$if parent
  ${W}/fA    = 0.01
  ${W}/fA_Re = fA_MaxValueOfIng     # footnote§
$else
  ${*}/fA    = fA_MaxValueOfIng
  ${*}/fA_Re = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_75-Re.inc]
[include:element_74-W.inc]
[include:element_73-Ta.inc]
