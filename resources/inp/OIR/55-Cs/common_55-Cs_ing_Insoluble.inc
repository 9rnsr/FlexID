[include:common_55-Cs_parameter.inc]

[parameter]
# ICRP Publ.137 p.142 Table 6.2 / Ingested materials, Relatively insoluble forms

$if parent
  ${Cs}/fA    = 0.1
  ${Cs}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_56-Ba.inc]
[include:element_55-Cs.inc]
