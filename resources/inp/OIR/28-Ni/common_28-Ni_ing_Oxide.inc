[include:common_28-Ni_parameter.inc]

[parameter]
  # ICRP Publ.151 p.138 Table 15.3 / Ingested materials, Nickel oxide

$if parent
  ${Ni}/fA    = 5E-4
  ${Ni}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_29-Cu.inc]
[include:element_28-Ni.inc]
[include:element_27-Co.inc]
