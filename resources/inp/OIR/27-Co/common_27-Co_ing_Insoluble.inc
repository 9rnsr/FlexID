[include:common_27-Co_parameter.inc]

[parameter]
  # ICRP Publ.134 p.156 Table 8.2 / Ingested materials, Insoluble forms

$if parent
  ${Co}/fA    = 5E-2
  ${Co}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_27-Co.inc]
[include:element_26-Fe.inc]
