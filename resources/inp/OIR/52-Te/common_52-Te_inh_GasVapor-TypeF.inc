[include:common_52-Te_parameter.inc]

[parameter]
  # ICRP Publ.137 p.72 Table 4.2 / All unspecified compounds

  $fr = 1           # Table 4.3 Type-F
  $sr = 50          # Table 4.3 Type-F
  $ss = 0           # Table 4.3 Type-F
  $fb = 0           # Table 4.3 footnote*
  $sb = 0           # Table 4.3 footnote*

$if parent
  ${Te}/fA    = 0.3                 # Table 4.3 Type-F
  ${Te}/fA_Re = fA_MaxValueOfIng    # Table 4.3 footnote¶
$else
  $fA    = fr * fA_MaxValueOfIng
  $fA_Re =      fA_MaxValueOfIng
$endif

[include:common_intake_inh_GasVapor.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_54-Xe.inc]
[include:element_53-I.inc]
[include:element_52-Te.inc]
[include:element_51-Sb.inc]
