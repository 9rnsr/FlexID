[include:common_52-Te_parameter.inc]

[parameter]
# ICRP Publ.137 p.73 Table 4.3 / Ingested materials, All forms

$if parent
  ${Te}/fA    = 0.3
  ${Te}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  $fA    = fA_MaxValueOfIng
  $fA_Re = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_54-Xe.inc]
[include:element_53-I.inc]
[include:element_52-Te.inc]
[include:element_51-Sb.inc]
