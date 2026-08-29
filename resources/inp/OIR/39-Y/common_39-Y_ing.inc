[include:common_39-Y_parameter.inc]

[parameter]
  # ICRP Publ.134 p.242 Table 11.2 / Ingested materials, All chemical forms

$if parent
  ${Y}/fA    = 1E-4
  ${Y}/fA_Re = fA_MaxValueOfIng     # footnote**
$else
  ${*}/fA    = fA_MaxValueOfIng
  ${*}/fA_Re = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_41-Nb.inc]
[include:element_40-Zr.inc]
[include:element_39-Y.inc]
[include:element_38-Sr.inc]
