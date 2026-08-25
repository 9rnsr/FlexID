[include:common_40-Zr_parameter.inc]

[parameter]
# ICRP Publ.134 p.264 Table 12.2 / Ingested materials, All chemical forms

$if parent
  ${Zr}/fA    = 0.002
  ${Zr}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_41-Nb.inc]
[include:element_40-Zr.inc]
[include:element_39-Y.inc]
[include:element_38-Sr.inc]
