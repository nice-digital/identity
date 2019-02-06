import React from "react";

const SvgPrint = props => (
	<svg
		width="1em"
		height="1em"
		viewBox="0 0 512 512"
		fill="none"
		className="icon"
		aria-hidden={true}
		{...props}
	>
		<path
			d="M419.128 153.288V59h-326.8v94.288H35v217.52h57.152v-59.44h326.96v59.44h57.344v-217.52h-57.344.016zm-42.864 0H135.176v-51.424h241.056v51.424h.032zm57.696 54.384c-7.168 0-12.976-5.792-12.976-12.96 0-7.168 5.808-12.96 12.976-12.96 7.168 0 12.976 5.792 12.976 12.96.016 7.168-5.808 12.96-12.976 12.96zm-58.384 244.56H135.192v-103.92H375.56v103.92h.016z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgPrint;
