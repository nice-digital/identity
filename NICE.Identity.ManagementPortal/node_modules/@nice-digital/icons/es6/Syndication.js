import React from "react";

const SvgSyndication = props => (
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
			d="M154.648 417.208c0 33.04-26.8 59.808-59.824 59.808-33.04 0-59.824-26.768-59.824-59.808s26.784-59.824 59.824-59.824c33.008 0 59.824 26.784 59.824 59.824zM35.016 185.656v88.608c111.312 1.152 201.6 91.456 202.752 202.752h88.608c-1.152-160.416-130.944-290.208-291.36-291.36zm0-62.032c94.096.4 182.48 37.2 249.04 103.792 66.72 66.704 103.536 155.328 103.808 249.6h88.608C475.912 233.32 278.632 35.864 35 35v88.624h.016z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgSyndication;
