[include:common_41-Nb_parameter.inc]

[parameter]
# ICRP Publ.134 p.288 Table 13.2 / Ingested material, All forms

$if parent
  ${Nb}/fA    = 0.01
  ${Nb}/fA_Re = 0.01        # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_41-Nb.inc]
[include:element_40-Zr.inc]
[include:element_39-Y.inc]
