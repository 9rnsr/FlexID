[include:common_34-Se_parameter.inc]

[parameter]
  # ICRP Publ.151 p.186 Table 20.2 / Ingested materials, Selenide and elemental selenium

$if parent
  ${Se}/fA    = 0.05
  ${Se}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_36-Kr.inc]
[include:element_35-Br.inc]
[include:element_34-Se.inc]
[include:element_33-As.inc]
