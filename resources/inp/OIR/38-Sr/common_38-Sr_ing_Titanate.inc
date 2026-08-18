[include:common_38-Sr_parameter.inc]

[parameter]
  # ICRP Publ.134 p.215 Table 10.2 / Ingested material, Strontium titanate

$if parent
  ${Sr}/fA    = 0.01
  ${Sr}/fA_Re = 0
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_39-Y.inc]
[include:element_38-Sr.inc]
[include:element_37-Rb.inc]
[include:element_36-Kr.inc]
