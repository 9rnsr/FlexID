[include:parameter_94-Pu_common.inc]

[parameter]
  # ICRP Publ.141 p.337 Table 22.11 / Ingested materials, Soluble forms

$if parent
  ${Pu}/fA     = 5E-4
  ${Pu}/fA_Re  = fA_MaxValueOfIng   # footnote§
$else
  ${^Rn}/fA    = fA_MaxValueOfIng
  ${^Rn}/fA_Re = fA_MaxValueOfIng
$endif
