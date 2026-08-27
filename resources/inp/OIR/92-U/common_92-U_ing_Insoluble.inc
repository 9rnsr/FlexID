[include:common_92-U_parameter.inc]

[parameter]
  # ICRP Publ.137 p.409 Table 15.2 / Ingested materials, Relatively insoluble forms

$if parent
  ${U}/fA      = 0.002
  ${U}/fA_Re   = fA_MaxValueOfIng   # footnote††
$else
  ${^Rn}/fA    = fA_MaxValueOfIng
  ${^Rn}/fA_Re = fA_MaxValueOfIng
$endif

[include:common_intake_ing.inc]

[include:common_HATM+UB.inc]

[include:element_94-Pu.inc]
[include:element_93-Np.inc]
[include:element_92-U.inc]
[include:element_91-Pa.inc]
[include:element_90-Th.inc]
[include:element_89-Ac.inc]
[include:element_88-Ra.inc]
[include:element_87-Fr.inc]
[include:element_86-Rn.inc]
[include:element_85-At.inc]
[include:element_84-Po.inc]
[include:element_83-Bi.inc]
[include:element_82-Pb.inc]
[include:element_81-Tl.inc]
