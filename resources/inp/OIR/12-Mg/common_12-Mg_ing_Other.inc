[include:common_12-Mg_parameter.inc]

[parameter]
  # ICRP Publ.151 p.45 Table 5.2 / Ingested materials, All other forms, unspecified forms

$if parent
  ${Mg}/fA    = 0.5
  ${Mg}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_13-Al.inc]
[include:element_12-Mg.inc]
