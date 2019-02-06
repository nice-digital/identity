import React from "react";

const SvgComment = props => (
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
			d="M469.453 85.667C469.453 62.2 450.467 43 427 43H85.667C62.2 43 43 62.2 43 85.667v256c0 23.466 19.2 42.666 42.667 42.666h298.666l85.334 85.334-.214-384zM384.333 299h-256v-42.667h256V299zm0-64h-256v-42.667h256V235zm0-64h-256v-42.667h256V171z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgComment;
