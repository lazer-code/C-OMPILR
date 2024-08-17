using testCompiler;

class Parser
{

    private static TokenTreeNode? ReorderBlock(TokenTreeNode? node, int bodyId = 0, TokenTreeNode? newTreeHead = null)
    {
        if (node == null)
            return null;

        var newRoot = new TokenTreeNode(node);

        for (int i = 0; i < node.Children.Count; i++)
        {
            var child = node.Children[i];

            if(child.Value.type == TokenType.Identifier_func)
                child.Value = new(child.Value.GetValue().Split(' ')[1], TokenType.Identifier_func);

            if (child.Value.type == TokenType.GreaterThan)
            {
                // Create a new node for the greater-than operator
                TokenTreeNode greaterThanNode = new(child);

                // Add previous and next siblings as children
                if (i < node.Children.Count - 1)
                    greaterThanNode.AddChild(node.Children[i + 1]);

                if (i > 0)
                    greaterThanNode.AddChild(node.Children[i - 1]);

                newRoot.AddChild(greaterThanNode);
                i++; // Skip the next sibling
            }

            else if(child.Value.type == TokenType.Keyword && child.Value.GetValue() == "return")
            {
                TokenTreeNode retNode = new(child);

                if(i < node.Children.Count -1)
                    retNode.AddChild(node.Children[i+1]);

                newRoot.AddChild(retNode);
                i++;
            }
            else if(child.Value.type == TokenType.Assignment)
            {
                TokenTreeNode assignmentNode = new(child);

                if(i < node.Children.Count -1)
                    assignmentNode.AddChild(node.Children[i + 1]);

                if(i > 0)
                    assignmentNode.AddChild(node.Children[i - 1]);

                i++;
                newRoot.AddChild(assignmentNode);
            }

            else
            {
                if(i != node.Children.Count -1)
                {
                    if(node.Children[i+1].Value.type == TokenType.GreaterThan)
                    {

                    }
                    else if(node.Children[i+1].Value.type == TokenType.Assignment)
                    {

                    }
                    else if(child.Value.type == TokenType.Keyword && child.Value.GetValue() == "int")
                    {

                    }
                    else
                    {
                        if(!child.IsArgument)
                            newRoot.AddChild(ReorderBlock(node.Children[i]));
                    }
                }

                else
                {
                    if(!child.IsArgument)
                        newRoot.AddChild(ReorderBlock(node.Children[i]));
                }
            }
        }

        return newRoot;
    }

    private static TokenTreeNode? FixIfs(TokenTreeNode? node)
    {
        if (node == null)
            return null;

        var newRoot = new TokenTreeNode(node);

        for (int i = 0; i < node.Children.Count; i++)
        {
            var child = node.Children[i];

            if (child.Value.type == TokenType.Keyword && child.Value.GetValue() == "if")
            {
                // Create a new node for the if statement
                var ifNode = new TokenTreeNode(child);

                // Safely add the next two children if they exist
                if (i + 1 < node.Children.Count)
                    ifNode.AddChild(FixIfs(node.Children[i + 1]));

                if (i + 2 < node.Children.Count)
                    ifNode.AddChild(FixIfs(node.Children[i + 2]));

                newRoot.AddChild(ifNode);
                i += 2; // Skip the next two children
            }

            else
                newRoot.AddChild(FixIfs(child));
        }   

        return newRoot;
    }

    public static TokenTreeNode? Parse(TokenTreeNode head)
    {
        TokenTreeNode? ttn = ReorderBlock(head);
        ttn = FixIfs(ttn);
        return ttn;
    }
}