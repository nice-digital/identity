import React from "react";

const SvgMinus = props => (
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
			d="M479 225.496v61.008c0 8.469-2.957 15.669-8.87 21.6-5.913 5.931-13.091 8.896-21.535 8.896H63.405c-8.444 0-15.622-2.965-21.535-8.896-5.913-5.931-8.87-13.131-8.87-21.6v-61.008c0-8.469 2.956-15.669 8.87-21.6C47.783 197.965 54.96 195 63.405 195h385.19c8.444 0 15.622 2.965 21.535 8.896 5.913 5.931 8.87 13.131 8.87 21.6z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgMinus;
