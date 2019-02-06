import React from "react";

const SvgStop = props => (
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
			d="M475.864 55.288v402.288c0 4.949-1.808 9.237-5.424 12.864-3.616 3.627-7.904 5.435-12.864 5.424H55.288c-4.95 0-9.237-1.808-12.864-5.424-3.627-3.616-5.435-7.904-5.424-12.864V55.288c0-4.95 1.808-9.237 5.424-12.864 3.616-3.627 7.904-5.435 12.864-5.424h402.288c4.949 0 9.237 1.808 12.864 5.424 3.627 3.616 5.435 7.904 5.424 12.864z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgStop;
