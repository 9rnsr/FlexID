[include:parameter_82-Pb_common.inc]

[parameter]
  # ICRP Publ.137 p.220 Table 9.2 / Ingested materials, All forms

$if parent
  ${Pb}/fA    = 0.2
  ${Pb}/fA_Re = fA_MaxValueOfIng    # footnote**
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif
