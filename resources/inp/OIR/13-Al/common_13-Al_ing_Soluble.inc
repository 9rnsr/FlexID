[include:common_13-Al_parameter.inc]

[parameter]
  # ICRP Publ.151 p.56 Table 6.6 / Ingested materials, Soluble forms

$if parent
  ${Al}/fA    = 0.003
  ${Al}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_13-Al.inc]
