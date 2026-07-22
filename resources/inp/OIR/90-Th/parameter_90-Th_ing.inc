[include:parameter_90-Th_common.inc]

[parameter]
  # ICRP Publ.137 p.362 Table 14.2 / Ingested materials, All forms

$if parent
  ${Th}/fA     = 5E-4
  ${Th}/fA_Re  = fA_MaxValueOfIng   # footnote**
$else
  ${^Rn}/fA    = fA_MaxValueOfIng
  ${^Rn}/fA_Re = fA_MaxValueOfIng
$endif
